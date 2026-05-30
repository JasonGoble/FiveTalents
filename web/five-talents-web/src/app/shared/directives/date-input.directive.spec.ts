import { Component } from '@angular/core';
import { TestBed, ComponentFixture } from '@angular/core/testing';
import { DateInputDirective } from './date-input.directive';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

@Component({
  standalone: true,
  imports: [
    DateInputDirective,
    MatDatepickerModule,
    MatNativeDateModule,
    MatInputModule,
    MatFormFieldModule,
  ],
  template: `
    <mat-form-field>
      <input matInput [matDatepicker]="picker" #input />
      <mat-datepicker #picker />
    </mat-form-field>
  `,
})
class HostComponent {}

function fireInputEvent(input: HTMLInputElement) {
  input.dispatchEvent(new Event('input', { bubbles: true }));
}

function firePaste(input: HTMLInputElement, text: string) {
  const event = Object.assign(new Event('paste', { bubbles: true, cancelable: true }), {
    clipboardData: { getData: () => text },
  });
  input.dispatchEvent(event);
}

describe('DateInputDirective', () => {
  let fixture: ComponentFixture<HostComponent>;
  let input: HTMLInputElement;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HostComponent, NoopAnimationsModule],
    }).compileComponents();
    fixture = TestBed.createComponent(HostComponent);
    fixture.detectChanges();
    input = fixture.nativeElement.querySelector('input') as HTMLInputElement;
  });

  describe('input formatting', () => {
    it('formats MM/DD/YYYY as digits are typed', () => {
      input.value = '01';
      fireInputEvent(input);
      expect(input.value).toBe('01');

      input.value = '0102';
      fireInputEvent(input);
      expect(input.value).toBe('01/02');

      input.value = '01021990';
      fireInputEvent(input);
      expect(input.value).toBe('01/02/1990');
    });

    it('caps at 8 digits (MM/DD/YYYY)', () => {
      input.value = '010219901234';
      fireInputEvent(input);
      expect(input.value).toBe('01/02/1990');
    });

    it('strips non-digit characters from input', () => {
      input.value = 'abc123';
      fireInputEvent(input);
      expect(input.value).toBe('12/3');
    });
  });

  describe('paste handling', () => {
    it('formats pasted digits into MM/DD/YYYY', () => {
      firePaste(input, '01021990');
      expect(input.value).toBe('01/02/1990');
    });

    it('strips non-digits from pasted text', () => {
      firePaste(input, '01-02-1990');
      expect(input.value).toBe('01/02/1990');
    });

    it('handles partial paste', () => {
      firePaste(input, '0102');
      expect(input.value).toBe('01/02');
    });
  });

  describe('keydown filtering', () => {
    it('prevents non-digit keys', () => {
      const event = new KeyboardEvent('keydown', { key: 'a', bubbles: true, cancelable: true });
      input.dispatchEvent(event);
      expect(event.defaultPrevented).toBe(true);
    });

    it('allows digit keys', () => {
      const event = new KeyboardEvent('keydown', { key: '5', bubbles: true, cancelable: true });
      input.dispatchEvent(event);
      expect(event.defaultPrevented).toBe(false);
    });

    it('allows Backspace', () => {
      const event = new KeyboardEvent('keydown', { key: 'Backspace', bubbles: true, cancelable: true });
      input.dispatchEvent(event);
      expect(event.defaultPrevented).toBe(false);
    });

    it('allows arrow keys', () => {
      const event = new KeyboardEvent('keydown', { key: 'ArrowLeft', bubbles: true, cancelable: true });
      input.dispatchEvent(event);
      expect(event.defaultPrevented).toBe(false);
    });

    it('allows Ctrl+key combinations', () => {
      const event = new KeyboardEvent('keydown', { key: 'v', ctrlKey: true, bubbles: true, cancelable: true });
      input.dispatchEvent(event);
      expect(event.defaultPrevented).toBe(false);
    });
  });
});
