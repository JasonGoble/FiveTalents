import { Component, DestroyRef, Input, OnInit, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ControlValueAccessor, FormControl, NG_VALUE_ACCESSOR, ReactiveFormsModule } from '@angular/forms';
import { MatAutocompleteModule, MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { Observable } from 'rxjs';
import { debounceTime, distinctUntilChanged, filter, startWith, switchMap, tap } from 'rxjs/operators';

export interface AutocompleteOption {
  id: number;
  label: string;
}

export type AutocompleteSearchFn = (query: string) => Observable<AutocompleteOption[]>;

@Component({
  selector: 'app-entity-autocomplete',
  standalone: true,
  imports: [ReactiveFormsModule, MatAutocompleteModule, MatFormFieldModule, MatInputModule],
  template: `
    <mat-form-field appearance="outline" class="full-width">
      <mat-label>{{ label }}</mat-label>
      <input matInput [formControl]="inputControl" [matAutocomplete]="auto" (blur)="onTouched()" />
      <mat-autocomplete #auto="matAutocomplete"
                        [displayWith]="displayWith"
                        (optionSelected)="onOptionSelected($event)">
        @if (loading()) {
          <mat-option disabled>Searching…</mat-option>
        } @else {
          @for (opt of options(); track opt.id) {
            <mat-option [value]="opt">{{ opt.label }}</mat-option>
          }
        }
      </mat-autocomplete>
    </mat-form-field>
  `,
  styles: [`.full-width { width: 100%; }`],
  providers: [{ provide: NG_VALUE_ACCESSOR, useExisting: EntityAutocompleteComponent, multi: true }],
})
export class EntityAutocompleteComponent implements ControlValueAccessor, OnInit {
  @Input({ required: true }) label!: string;
  @Input({ required: true }) searchFn!: AutocompleteSearchFn;

  inputControl = new FormControl<string | AutocompleteOption>('');
  options = signal<AutocompleteOption[]>([]);
  loading = signal(false);

  private _onChange: (value: number | null) => void = () => {};
  protected onTouched: () => void = () => {};

  private destroyRef = inject(DestroyRef);

  ngOnInit() {
    this.inputControl.valueChanges.pipe(
      startWith(''),
      filter((v): v is string => typeof v === 'string'),
      debounceTime(250),
      distinctUntilChanged(),
      tap(() => this.loading.set(true)),
      switchMap(q => this.searchFn(q)),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe(results => {
      this.options.set(results);
      this.loading.set(false);
    });
  }

  displayWith = (opt: AutocompleteOption | null): string => opt?.label ?? '';

  onOptionSelected(event: MatAutocompleteSelectedEvent): void {
    const opt = event.option.value as AutocompleteOption;
    this._onChange(opt.id);
    this.onTouched();
  }

  writeValue(id: number | null): void {
    if (!id) this.inputControl.setValue('', { emitEvent: false });
  }

  registerOnChange(fn: (value: number | null) => void): void { this._onChange = fn; }
  registerOnTouched(fn: () => void): void { this.onTouched = fn; }
  setDisabledState(isDisabled: boolean): void {
    isDisabled ? this.inputControl.disable({ emitEvent: false }) : this.inputControl.enable({ emitEvent: false });
  }
}
