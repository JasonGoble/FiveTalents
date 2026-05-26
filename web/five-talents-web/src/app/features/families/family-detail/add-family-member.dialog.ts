import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { firstValueFrom } from 'rxjs';
import { FamilyRole } from '../../../core/models/family.models';
import { MemberStatus } from '../../../core/models/member.models';
import { FamilyService } from '../../../core/services/family.service';
import { MemberService } from '../../../core/services/member.service';

export interface AddFamilyMemberDialogData {
  familyId: number;
  organizationId: number;
  roles: FamilyRole[];
}

interface MemberOption { id: number; fullName: string; }

@Component({
  selector: 'app-add-family-member-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, MatButtonModule, MatDialogModule,
            MatFormFieldModule, MatInputModule, MatSelectModule],
  template: `
    <h2 mat-dialog-title>Add Member to Family</h2>
    <mat-dialog-content>
      <div class="form-col">
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Member</mat-label>
          <mat-select [(ngModel)]="selectedMemberId">
            @for (m of members(); track m.id) {
              <mat-option [value]="m.id">{{ m.fullName }}</mat-option>
            }
          </mat-select>
        </mat-form-field>
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Role</mat-label>
          <mat-select [(ngModel)]="selectedRoleId">
            @for (r of data.roles; track r.id) {
              <mat-option [value]="r.id">{{ r.name }}</mat-option>
            }
          </mat-select>
        </mat-form-field>
      </div>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close>Cancel</button>
      <button mat-flat-button color="primary"
              [disabled]="!selectedMemberId || !selectedRoleId || saving()"
              (click)="save()">
        {{ saving() ? 'Adding…' : 'Add' }}
      </button>
    </mat-dialog-actions>
  `,
  styles: [`.full-width { width: 100%; } .form-col { display: flex; flex-direction: column; gap: 4px; padding-top: 8px; }`],
})
export class AddFamilyMemberDialogComponent implements OnInit {
  readonly data = inject<AddFamilyMemberDialogData>(MAT_DIALOG_DATA);
  private dialogRef = inject(MatDialogRef<AddFamilyMemberDialogComponent>);
  private familyService = inject(FamilyService);
  private memberService = inject(MemberService);

  members = signal<MemberOption[]>([]);
  selectedMemberId: number | null = null;
  selectedRoleId: number | null = null;
  saving = signal(false);

  ngOnInit() {
    if (this.data.roles.length) this.selectedRoleId = this.data.roles[0].id;
    this.loadMembers();
  }

  private async loadMembers() {
    const result = await firstValueFrom(
      this.memberService.getAll(this.data.organizationId, 1, 500, undefined, MemberStatus.Active)
    );
    this.members.set(result.items.map(m => ({ id: m.id, fullName: m.fullName })));
  }

  async save() {
    if (!this.selectedMemberId || !this.selectedRoleId) return;
    this.saving.set(true);
    try {
      await firstValueFrom(this.familyService.addMember(this.data.familyId, {
        memberId: this.selectedMemberId,
        roleId: this.selectedRoleId,
      }));
      this.dialogRef.close(true);
    } finally {
      this.saving.set(false);
    }
  }
}
