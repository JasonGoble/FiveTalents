import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { firstValueFrom, map } from 'rxjs';
import { BreakpointObserver } from '@angular/cdk/layout';
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatSelectModule } from '@angular/material/select';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Group, GroupStatus } from '../../core/models/group.models';
import { Organization } from '../../core/models/organization.models';
import { GroupService } from '../../core/services/group.service';
import { OrganizationService } from '../../core/services/organization.service';
import { AuthService } from '../../core/services/auth.service';
import { GroupFormDialogComponent } from './group-form.dialog';

const STATUS_FILTERS: { value: GroupStatus | null; label: string }[] = [
  { value: null,        label: 'All' },
  { value: 'Active',    label: 'Active' },
  { value: 'Forming',   label: 'Forming' },
  { value: 'Inactive',  label: 'Inactive' },
  { value: 'Disbanded', label: 'Disbanded' },
];

const FREQUENCY_LABELS: Record<string, string> = {
  Weekly:    'Weekly',
  BiWeekly:  'Every other week',
  Monthly:   'Monthly',
  Quarterly: 'Quarterly',
  AsNeeded:  'As needed',
};

const STATUS_COLORS: Record<GroupStatus, { bg: string; color: string }> = {
  Active:    { bg: '#e8f5e9', color: '#2e7d32' },
  Forming:   { bg: '#e3f2fd', color: '#1565c0' },
  Inactive:  { bg: '#f5f5f5', color: '#757575' },
  Disbanded: { bg: '#fce4ec', color: '#c62828' },
};

@Component({
  selector: 'app-groups',
  standalone: true,
  imports: [
    CommonModule, MatButtonModule, MatButtonToggleModule, MatCardModule,
    MatChipsModule, MatDialogModule, MatFormFieldModule, MatIconModule,
    MatInputModule, MatProgressSpinnerModule, MatTooltipModule, MatSelectModule,
  ],
  templateUrl: './groups.component.html',
  styleUrl: './groups.component.scss',
})
export class GroupsComponent implements OnInit {
  private groupService = inject(GroupService);
  private orgService = inject(OrganizationService);
  private authService = inject(AuthService);
  private dialog = inject(MatDialog);
  private bp = inject(BreakpointObserver);

  isMobile = toSignal(
    this.bp.observe('(max-width: 767px)').pipe(map(r => r.matches)),
    { initialValue: false }
  );

  isAdmin = computed(() => this.authService.currentUser()?.isSystemAdmin ?? false);
  orgs = signal<Organization[]>([]);
  selectedOrgId = signal<number | null>(null);

  readonly statusFilters = STATUS_FILTERS;

  loading = signal(false);
  searchText = signal('');
  statusFilter = signal<GroupStatus | null>(null);
  groups = signal<Group[]>([]);

  filtered = computed(() => {
    let list = this.groups();
    const q = this.searchText().toLowerCase().trim();
    const s = this.statusFilter();
    if (q) list = list.filter(g =>
      g.name.toLowerCase().includes(q) ||
      (g.description ?? '').toLowerCase().includes(q)
    );
    if (s) list = list.filter(g => g.status === s);
    return list;
  });

  private get orgId(): number | null {
    return this.isAdmin()
      ? this.selectedOrgId()
      : (this.authService.currentUser()?.primaryOrganizationId ?? null);
  }

  ngOnInit() {
    if (this.isAdmin()) {
      this.orgService.getAll().subscribe(orgs => this.orgs.set(orgs));
    }
    this.load();
  }

  onOrgChange(orgId: number | null) {
    this.selectedOrgId.set(orgId);
    this.load();
  }

  private async load() {
    if (!this.isAdmin() && !this.orgId) return;
    this.loading.set(true);
    try {
      const groups = await firstValueFrom(this.groupService.getAll(this.orgId));
      this.groups.set(groups);
    } finally {
      this.loading.set(false);
    }
  }

  openForm(group?: Group) {
    const orgId = this.orgId ?? this.authService.currentUser()?.primaryOrganizationId ?? 0;
    const ref = this.dialog.open(GroupFormDialogComponent, {
      width: '620px',
      maxWidth: '95vw',
      data: { organizationId: orgId, group },
    });
    ref.afterClosed().subscribe(saved => { if (saved) this.load(); });
  }

  async deleteGroup(group: Group) {
    if (!confirm(`Delete "${group.name}"? This cannot be undone.`)) return;
    await firstValueFrom(this.groupService.delete(group.id));
    this.load();
  }

  statusStyle(status: GroupStatus) {
    return STATUS_COLORS[status] ?? { bg: '#f5f5f5', color: '#757575' };
  }

  frequencyLabel(freq: string): string {
    return FREQUENCY_LABELS[freq] ?? freq;
  }

  formatTime(hhmm: string): string {
    const [h, m] = hhmm.split(':').map(Number);
    const ampm = h < 12 ? 'AM' : 'PM';
    const hour = h === 0 ? 12 : h > 12 ? h - 12 : h;
    return `${hour}:${m.toString().padStart(2, '0')} ${ampm}`;
  }
}
