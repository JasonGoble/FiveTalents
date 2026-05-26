import { Component, inject, OnInit, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { firstValueFrom, map } from 'rxjs';
import { BreakpointObserver } from '@angular/cdk/layout';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Family, FamilyMember, FamilyRole } from '../../../core/models/family.models';
import { FamilyService } from '../../../core/services/family.service';
import { AuthService } from '../../../core/services/auth.service';
import { AddFamilyMemberDialogComponent } from './add-family-member.dialog';
import { FamilyFormDialogComponent } from '../family-form.dialog';

@Component({
  selector: 'app-family-detail',
  standalone: true,
  imports: [
    CommonModule, RouterModule, MatButtonModule, MatCardModule, MatDialogModule,
    MatIconModule, MatProgressSpinnerModule, MatSelectModule, MatTableModule, MatTooltipModule,
  ],
  templateUrl: './family-detail.component.html',
  styleUrl: './family-detail.component.scss',
})
export class FamilyDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private familyService = inject(FamilyService);
  private authService = inject(AuthService);
  private dialog = inject(MatDialog);
  private bp = inject(BreakpointObserver);

  isMobile = toSignal(
    this.bp.observe('(max-width: 767px)').pipe(map(r => r.matches)),
    { initialValue: false }
  );

  loading = signal(false);
  family = signal<Family | null>(null);
  roles = signal<FamilyRole[]>([]);

  readonly displayedColumns = ['name', 'role', 'actions'];

  private get familyId(): number {
    return Number(this.route.snapshot.paramMap.get('id'));
  }

  private get orgId(): number {
    return this.authService.currentUser()?.primaryOrganizationId ?? 0;
  }

  ngOnInit() { this.load(); }

  private async load() {
    this.loading.set(true);
    try {
      const [family, roles] = await Promise.all([
        firstValueFrom(this.familyService.getById(this.familyId)),
        firstValueFrom(this.familyService.getRoles(this.orgId)),
      ]);
      this.family.set(family);
      this.roles.set(roles);
    } finally {
      this.loading.set(false);
    }
  }

  openEditForm() {
    const f = this.family();
    if (!f) return;
    this.dialog.open(FamilyFormDialogComponent, {
      width: '480px',
      maxWidth: '95vw',
      data: { organizationId: f.organizationId, family: { id: f.id, name: f.name, memberCount: f.members.length, organizationId: f.organizationId } },
    }).afterClosed().subscribe(saved => { if (saved) this.load(); });
  }

  openAddMember() {
    this.dialog.open(AddFamilyMemberDialogComponent, {
      width: '540px',
      maxWidth: '95vw',
      data: { familyId: this.familyId, organizationId: this.orgId, roles: this.roles() },
    }).afterClosed().subscribe(saved => { if (saved) this.load(); });
  }

  async updateRole(member: FamilyMember, roleId: number) {
    await firstValueFrom(this.familyService.updateMemberRole(this.familyId, member.memberId, { roleId }));
    this.load();
  }

  async removeMember(member: FamilyMember) {
    if (!confirm(`Remove ${member.fullName} from this family?`)) return;
    await firstValueFrom(this.familyService.removeMember(this.familyId, member.memberId));
    this.load();
  }

  async deleteFamily() {
    const f = this.family();
    if (!f || !confirm(`Delete "${f.name}"? This cannot be undone.`)) return;
    await firstValueFrom(this.familyService.delete(f.id));
    this.router.navigate(['/families']);
  }
}
