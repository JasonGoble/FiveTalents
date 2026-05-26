import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { firstValueFrom } from 'rxjs';
import { FamilySummary } from '../../core/models/family.models';
import { FamilyService } from '../../core/services/family.service';

export interface FamilyFormDialogData {
  organizationId: number;
  family?: FamilySummary;
}

@Component({
  selector: 'app-family-form-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, MatButtonModule, MatDialogModule, MatFormFieldModule, MatInputModule],
  template: `
    <h2 mat-dialog-title>{{ data.family ? 'Edit' : 'Add' }} Family</h2>
    <mat-dialog-content>
      <mat-form-field appearance="outline" class="full-width">
        <mat-label>Family Name</mat-label>
        <input matInput [(ngModel)]="name" placeholder="e.g. The Smith Family" required />
      </mat-form-field>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close>Cancel</button>
      <button mat-flat-button color="primary" [disabled]="!name().trim() || saving()" (click)="save()">
        {{ saving() ? 'Saving…' : 'Save' }}
      </button>
    </mat-dialog-actions>
  `,
  styles: [`.full-width { width: 100%; margin-top: 8px; }`],
})
export class FamilyFormDialogComponent implements OnInit {
  readonly data = inject<FamilyFormDialogData>(MAT_DIALOG_DATA);
  private dialogRef = inject(MatDialogRef<FamilyFormDialogComponent>);
  private familyService = inject(FamilyService);

  name = signal('');
  saving = signal(false);

  ngOnInit() {
    if (this.data.family) this.name.set(this.data.family.name);
  }

  async save() {
    if (!this.name().trim()) return;
    this.saving.set(true);
    try {
      if (this.data.family) {
        await firstValueFrom(this.familyService.update(this.data.family.id, {
          id: this.data.family.id,
          name: this.name().trim(),
        }));
      } else {
        await firstValueFrom(this.familyService.create({
          organizationId: this.data.organizationId,
          name: this.name().trim(),
        }));
      }
      this.dialogRef.close(true);
    } finally {
      this.saving.set(false);
    }
  }
}
