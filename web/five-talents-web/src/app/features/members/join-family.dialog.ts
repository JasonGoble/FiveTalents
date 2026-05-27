import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
import { firstValueFrom } from 'rxjs';
import { AutocompleteSearchFn, EntityAutocompleteComponent } from '../../shared/components/entity-autocomplete/entity-autocomplete.component';
import { FamilyRole, FamilySummary } from '../../core/models/family.models';
import { FamilyService } from '../../core/services/family.service';

export interface JoinFamilyDialogData {
  memberId: number;
  organizationId: number;
}

@Component({
  selector: 'app-join-family-dialog',
  standalone: true,
  imports: [FormsModule, MatButtonModule, MatDialogModule,
            MatFormFieldModule, MatSelectModule, EntityAutocompleteComponent],
  template: `
    <h2 mat-dialog-title>Add to Family</h2>
    <mat-dialog-content>
      <div class="form-col">
        <app-entity-autocomplete
          label="Family"
          [searchFn]="searchFamilies"
          [(ngModel)]="selectedFamilyId" />
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Role</mat-label>
          <mat-select [(ngModel)]="selectedRoleId">
            @for (r of roles(); track r.id) {
              <mat-option [value]="r.id">{{ r.name }}</mat-option>
            }
          </mat-select>
        </mat-form-field>
      </div>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close>Cancel</button>
      <button mat-flat-button color="primary"
              [disabled]="!selectedFamilyId || !selectedRoleId || saving()"
              (click)="save()">
        {{ saving() ? 'Adding…' : 'Add' }}
      </button>
    </mat-dialog-actions>
  `,
  styles: [`.full-width { width: 100%; } .form-col { display: flex; flex-direction: column; gap: 4px; padding-top: 8px; }`],
})
export class JoinFamilyDialogComponent implements OnInit {
  readonly data = inject<JoinFamilyDialogData>(MAT_DIALOG_DATA);
  private dialogRef = inject(MatDialogRef<JoinFamilyDialogComponent>);
  private familyService = inject(FamilyService);

  private allFamilies = signal<FamilySummary[]>([]);
  roles = signal<FamilyRole[]>([]);
  selectedFamilyId: number | null = null;
  selectedRoleId: number | null = null;
  saving = signal(false);

  searchFamilies: AutocompleteSearchFn = (query) => {
    const q = query.toLowerCase();
    return of(
      this.allFamilies()
        .filter(f => !q || f.name.toLowerCase().includes(q))
        .map(f => ({ id: f.id, label: f.name }))
    );
  };

  ngOnInit() { this.load(); }

  private async load() {
    const [families, roles] = await Promise.all([
      firstValueFrom(this.familyService.getAll(this.data.organizationId)),
      firstValueFrom(this.familyService.getRoles(this.data.organizationId)),
    ]);
    this.allFamilies.set(families);
    this.roles.set(roles);
    if (roles.length) this.selectedRoleId = roles[0].id;
  }

  async save() {
    if (!this.selectedFamilyId || !this.selectedRoleId) return;
    this.saving.set(true);
    try {
      await firstValueFrom(this.familyService.addMember(this.selectedFamilyId, {
        memberId: this.data.memberId,
        roleId: this.selectedRoleId,
      }));
      this.dialogRef.close(true);
    } finally {
      this.saving.set(false);
    }
  }
}
