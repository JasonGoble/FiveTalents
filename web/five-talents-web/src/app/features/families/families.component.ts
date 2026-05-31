import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { firstValueFrom, map } from 'rxjs';
import { BreakpointObserver } from '@angular/cdk/layout';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { FamilySummary } from '../../core/models/family.models';
import { Organization } from '../../core/models/organization.models';
import { FamilyService } from '../../core/services/family.service';
import { OrganizationService } from '../../core/services/organization.service';
import { AuthService } from '../../core/services/auth.service';
import { FamilyFormDialogComponent } from './family-form.dialog';

@Component({
  selector: 'app-families',
  standalone: true,
  imports: [
    CommonModule, RouterModule, MatButtonModule, MatCardModule, MatDialogModule,
    MatFormFieldModule, MatIconModule, MatInputModule, MatProgressSpinnerModule,
    MatSelectModule, MatTableModule, MatTooltipModule,
  ],
  templateUrl: './families.component.html',
  styleUrl: './families.component.scss',
})
export class FamiliesComponent implements OnInit {
  private familyService = inject(FamilyService);
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

  loading = signal(false);
  searchText = signal('');
  families = signal<FamilySummary[]>([]);

  filtered = computed(() => {
    const q = this.searchText().toLowerCase().trim();
    return q
      ? this.families().filter(f => f.name.toLowerCase().includes(q))
      : this.families();
  });

  get displayedColumns(): string[] {
    return this.isAdmin() && this.selectedOrgId() === null
      ? ['name', 'orgName', 'memberCount', 'actions']
      : ['name', 'memberCount', 'actions'];
  }

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
      this.families.set(await firstValueFrom(this.familyService.getAll(this.orgId)));
    } finally {
      this.loading.set(false);
    }
  }

  openForm(family?: FamilySummary) {
    const orgId = this.orgId ?? this.authService.currentUser()?.primaryOrganizationId ?? 0;
    this.dialog.open(FamilyFormDialogComponent, {
      width: '480px',
      maxWidth: '95vw',
      data: { organizationId: orgId, family },
    }).afterClosed().subscribe(saved => { if (saved) this.load(); });
  }

  async deleteFamily(family: FamilySummary) {
    if (!confirm(`Delete "${family.name}"? This cannot be undone.`)) return;
    await firstValueFrom(this.familyService.delete(family.id));
    this.load();
  }
}
