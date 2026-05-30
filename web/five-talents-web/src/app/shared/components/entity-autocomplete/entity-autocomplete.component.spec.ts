import { TestBed } from '@angular/core/testing';
import { render, screen } from '@testing-library/angular';
import userEvent from '@testing-library/user-event';
import { of } from 'rxjs';
import { EntityAutocompleteComponent, AutocompleteOption } from './entity-autocomplete.component';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

const mockOptions: AutocompleteOption[] = [
  { id: 1, label: 'Alice Johnson' },
  { id: 2, label: 'Bob Smith' },
];

describe('EntityAutocompleteComponent', () => {
  const searchFn = vi.fn().mockReturnValue(of(mockOptions));

  beforeEach(() => searchFn.mockReturnValue(of(mockOptions)));

  it('renders the label', async () => {
    await render(EntityAutocompleteComponent, {
      imports: [NoopAnimationsModule],
      componentInputs: { label: 'Select Member', searchFn },
    });
    expect(screen.getByLabelText('Select Member')).toBeTruthy();
  });

  it('shows options after typing', async () => {
    const user = userEvent.setup();
    await render(EntityAutocompleteComponent, {
      imports: [NoopAnimationsModule],
      componentInputs: { label: 'Member', searchFn },
    });
    const input = screen.getByRole('combobox');
    await user.type(input, 'Ali');
    const options = await screen.findAllByRole('option');
    expect(options.length).toBe(2);
    expect(options[0].textContent).toContain('Alice Johnson');
  });

  it('starts with loading=false and options empty', async () => {
    searchFn.mockReturnValue(of([]));
    const { fixture } = await render(EntityAutocompleteComponent, {
      imports: [NoopAnimationsModule],
      componentInputs: { label: 'Member', searchFn },
    });
    const component = fixture.componentInstance;
    expect(component.loading()).toBe(false);
    expect(component.options()).toEqual([]);
  });

  it('disables input when setDisabledState(true) called', async () => {
    const { fixture } = await render(EntityAutocompleteComponent, {
      imports: [NoopAnimationsModule],
      componentInputs: { label: 'Member', searchFn },
    });
    const component = fixture.componentInstance;
    component.setDisabledState(true);
    expect(component.inputControl.disabled).toBe(true);
  });

  it('re-enables input when setDisabledState(false) called', async () => {
    const { fixture } = await render(EntityAutocompleteComponent, {
      imports: [NoopAnimationsModule],
      componentInputs: { label: 'Member', searchFn },
    });
    const component = fixture.componentInstance;
    component.setDisabledState(true);
    component.setDisabledState(false);
    expect(component.inputControl.enabled).toBe(true);
  });

  it('writeValue with null clears the input', async () => {
    const { fixture } = await render(EntityAutocompleteComponent, {
      imports: [NoopAnimationsModule],
      componentInputs: { label: 'Member', searchFn },
    });
    const component = fixture.componentInstance;
    component.writeValue(null);
    expect(component.inputControl.value).toBe('');
  });

  it('displayWith returns empty string for null', async () => {
    const { fixture } = await render(EntityAutocompleteComponent, {
      imports: [NoopAnimationsModule],
      componentInputs: { label: 'Member', searchFn },
    });
    expect(fixture.componentInstance.displayWith(null)).toBe('');
  });

  it('displayWith returns label for a valid option', async () => {
    const { fixture } = await render(EntityAutocompleteComponent, {
      imports: [NoopAnimationsModule],
      componentInputs: { label: 'Member', searchFn },
    });
    expect(fixture.componentInstance.displayWith({ id: 1, label: 'Alice Johnson' })).toBe('Alice Johnson');
  });
});
