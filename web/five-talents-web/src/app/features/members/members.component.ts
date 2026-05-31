import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged, map } from 'rxjs';
import { BreakpointObserver } from '@angular/cdk/layout';

import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';

import { MemberService } from '../../core/services/member.service';
import { OrganizationService } from '../../core/services/organization.service';
import { AuthService } from '../../core/services/auth.service';
import { MemberStatus, MemberSummary } from '../../core/models/member.models';
import { Organization } from '../../core/models/organization.models';

@Component({
  selector: 'app-members',
  standalone: true,
  imports: [
    CommonModule, RouterModule, ReactiveFormsModule,
    MatTableModule, MatPaginatorModule, MatInputModule, MatFormFieldModule,
    MatButtonModule, MatIconModule, MatSelectModule, MatProgressSpinnerModule,
    MatTooltipModule
  ],
  templateUrl: './members.component.html',
  styleUrl: './members.component.scss'
})
export class MembersComponent implements OnInit {
  private memberService = inject(MemberService);
  private orgService = inject(OrganizationService);
  private auth = inject(AuthService);
  private router = inject(Router);
  private bp = inject(BreakpointObserver);

  isMobile = toSignal(
    this.bp.observe('(max-width: 767px)').pipe(map(r => r.matches)),
    { initialValue: false }
  );

  isAdmin = computed(() => this.auth.currentUser()?.isSystemAdmin ?? false);
  orgs = signal<Organization[]>([]);
  selectedOrgId = signal<number | null>(null);

  get columns() {
    const showOrg = this.isAdmin() && this.selectedOrgId() === null;
    return showOrg
      ? ['fullName', 'orgName', 'primaryEmail', 'primaryPhone', 'status', 'actions']
      : ['fullName', 'primaryEmail', 'primaryPhone', 'status', 'actions'];
  }

  members = signal<MemberSummary[]>([]);
  totalCount = signal(0);
  loading = signal(false);
  pageSize = 25;
  pageNumber = 1;

  searchControl = new FormControl('');
  statusControl = new FormControl<number | null>(null);

  ngOnInit() {
    if (this.isAdmin()) {
      this.orgService.getAll().subscribe(orgs => this.orgs.set(orgs));
    }
    this.load();
    this.searchControl.valueChanges.pipe(debounceTime(400), distinctUntilChanged())
      .subscribe(() => { this.pageNumber = 1; this.load(); });
    this.statusControl.valueChanges
      .subscribe(() => { this.pageNumber = 1; this.load(); });
  }

  onOrgChange(orgId: number | null) {
    this.selectedOrgId.set(orgId);
    this.pageNumber = 1;
    this.load();
  }

  load() {
    const orgId = this.isAdmin()
      ? this.selectedOrgId()
      : (this.auth.currentUser()?.primaryOrganizationId ?? null);
    if (!this.isAdmin() && !orgId) return;
    this.loading.set(true);
    this.memberService.getAll(orgId, this.pageNumber, this.pageSize,
      this.searchControl.value ?? undefined,
      this.statusControl.value ?? undefined
    ).subscribe({
      next: result => {
        this.members.set(result.items);
        this.totalCount.set(result.totalCount);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  onPage(event: PageEvent) {
    this.pageNumber = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.load();
  }

  view(id: number) { this.router.navigate(['/members', id]); }
  edit(id: number) { this.router.navigate(['/members', id, 'edit']); }

  statusLabel(status: MemberStatus | string) {
    return (status as string) ?? 'Unknown';
  }
  statusClass(status: MemberStatus | string) {
    return ((status as string) ?? '').toLowerCase();
  }
}
