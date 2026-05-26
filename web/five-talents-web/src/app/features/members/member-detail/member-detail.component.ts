import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MemberService } from '../../../core/services/member.service';
import { FamilyService } from '../../../core/services/family.service';
import { AuthService } from '../../../core/services/auth.service';
import { Member, MemberStatus } from '../../../core/models/member.models';
import { MemberFamilyMembership } from '../../../core/models/family.models';
import { LinkUserDialogComponent, LinkUserDialogData } from '../link-user.dialog';
import { MoveOrganizationDialogComponent, MoveOrganizationDialogData } from '../move-organization.dialog';
import { JoinFamilyDialogComponent, JoinFamilyDialogData } from '../join-family.dialog';
import { UserSummary } from '../../../core/models/api.models';

@Component({
  selector: 'app-member-detail',
  standalone: true,
  imports: [
    CommonModule, RouterModule, MatButtonModule, MatIconModule,
    MatCardModule, MatProgressSpinnerModule, MatDividerModule,
    MatDialogModule, MatSnackBarModule, MatSlideToggleModule, MatChipsModule,
    MatTooltipModule,
  ],
  templateUrl: './member-detail.component.html',
  styleUrl: './member-detail.component.scss'
})
export class MemberDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private memberService = inject(MemberService);
  private familyService = inject(FamilyService);
  private authService = inject(AuthService);
  private snackBar = inject(MatSnackBar);
  private dialog = inject(MatDialog);

  member = signal<Member | null>(null);
  families = signal<MemberFamilyMembership[]>([]);
  loading = signal(false);

  isOwnProfile = computed(() => {
    const user = this.authService.currentUser();
    const m = this.member();
    return !!user && !!m && user.memberId === m.id;
  });

  isAdmin = computed(() => !!this.authService.currentUser()?.isSystemAdmin);

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.loading.set(true);
    this.memberService.getById(id).subscribe({
      next: m => {
        this.member.set(m);
        this.loading.set(false);
        this.loadFamilies(m.id);
      },
      error: () => { this.loading.set(false); this.router.navigate(['/members']); }
    });
  }

  private loadFamilies(memberId: number) {
    this.familyService.getMemberships(memberId).subscribe(f => this.families.set(f));
  }

  openJoinFamily(m: Member) {
    this.dialog.open(JoinFamilyDialogComponent, {
      width: '480px',
      maxWidth: '95vw',
      data: { memberId: m.id, organizationId: m.organizationId } satisfies JoinFamilyDialogData,
    }).afterClosed().subscribe(saved => { if (saved) this.loadFamilies(m.id); });
  }

  async leaveFamily(m: Member, membership: MemberFamilyMembership) {
    if (!confirm(`Remove ${m.firstName} from "${membership.familyName}"?`)) return;
    this.familyService.removeMember(membership.familyId, m.id).subscribe({
      next: () => this.loadFamilies(m.id),
      error: () => this.snackBar.open('Failed to remove from family', 'OK', { duration: 3000 }),
    });
  }

  confirmDelete() {
    if (!confirm(`Delete ${this.member()?.firstName} ${this.member()?.lastName}? This cannot be undone.`)) return;
    this.memberService.delete(this.member()!.id).subscribe({
      next: () => {
        this.snackBar.open('Member deleted', 'OK', { duration: 3000 });
        this.router.navigate(['/members']);
      },
      error: () => this.snackBar.open('Failed to delete member', 'OK', { duration: 3000 })
    });
  }

  linkUser(m: Member) {
    const ref = this.dialog.open(LinkUserDialogComponent, {
      width: '480px', maxWidth: '95vw',
      data: { memberName: `${m.firstName} ${m.lastName}`, memberEmail: m.emails?.find(e => e.isPrimary)?.email ?? m.emails?.[0]?.email } satisfies LinkUserDialogData
    });
    ref.afterClosed().subscribe((user: UserSummary | undefined) => {
      if (!user) return;
      this.memberService.linkUser(m.id, user.id).subscribe({
        next: () => {
          this.snackBar.open('User linked successfully', 'OK', { duration: 3000 });
          this.reload();
        },
        error: (err) => this.snackBar.open(err.error?.title ?? 'Failed to link user', 'OK', { duration: 4000 })
      });
    });
  }

  unlinkUser(m: Member) {
    if (!confirm('Unlink this member from their user account?')) return;
    this.memberService.unlinkUser(m.id).subscribe({
      next: () => {
        this.snackBar.open('User unlinked', 'OK', { duration: 3000 });
        this.reload();
      },
      error: () => this.snackBar.open('Failed to unlink user', 'OK', { duration: 3000 })
    });
  }

  moveOrg(m: Member) {
    const ref = this.dialog.open(MoveOrganizationDialogComponent, {
      width: '440px', maxWidth: '95vw',
      data: { memberName: `${m.firstName} ${m.lastName}`, currentOrgId: m.organizationId, currentOrgName: m.orgName } satisfies MoveOrganizationDialogData
    });
    ref.afterClosed().subscribe((orgId: number | undefined) => {
      if (!orgId) return;
      this.memberService.moveOrganization(m.id, orgId).subscribe({
        next: () => {
          this.snackBar.open('Member moved successfully', 'OK', { duration: 3000 });
          this.reload();
        },
        error: (err) => this.snackBar.open(err.error?.title ?? 'Failed to move member', 'OK', { duration: 4000 })
      });
    });
  }

  invite(m: Member) {
    this.memberService.invite(m.id, window.location.origin).subscribe({
      next: () => {
          const email = m.emails?.find(e => e.isPrimary)?.email ?? m.emails?.[0]?.email ?? 'member';
          this.snackBar.open(`Invite sent to ${email}`, 'OK', { duration: 4000 });
        },
      error: (err) => this.snackBar.open(err.error?.title ?? 'Failed to send invite', 'OK', { duration: 4000 })
    });
  }

  updatePrivacy(m: Member, field: 'sharePhoneWithNetwork' | 'shareEmailWithNetwork' | 'shareAddressWithNetwork', value: boolean) {
    this.memberService.update(m.id, { [field]: value }).subscribe({
      next: () => {
        this.member.set({ ...m, [field]: value });
        this.snackBar.open('Privacy setting updated', 'OK', { duration: 2000 });
      },
      error: () => this.snackBar.open('Failed to update privacy setting', 'OK', { duration: 3000 })
    });
  }

  private reload() {
    this.memberService.getById(this.member()!.id).subscribe(m => this.member.set(m));
  }

  statusLabel(status: MemberStatus | string) {
    return (status as string) ?? 'Unknown';
  }
  statusClass(status: MemberStatus | string) {
    return ((status as string) ?? '').toLowerCase();
  }
}
