import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'phone',
})
export class PhonePipe implements PipeTransform {
  transform(
    rawPhone: string | null | undefined,
    format: 'national' | 'international' = 'national',
  ): string {
    if (!rawPhone) return '';

    // Strip all non-digit characters
    const digits = rawPhone.replace(/\D/g, '');

    // Extract the base 10-digit number (handles '18095550199' or '8095550199')
    const cleanNumber = digits.length === 11 && digits.startsWith('1') ? digits.slice(1) : digits;

    if (cleanNumber.length !== 10) {
      return rawPhone; // Fallback to raw value if it does not fit a standard 10-digit number
    }

    const areaCode = cleanNumber.slice(0, 3);
    const prefix = cleanNumber.slice(3, 6);
    const line = cleanNumber.slice(6, 10);

    if (format === 'international') {
      return `+1 (${areaCode}) ${prefix}-${line}`;
    }

    // Default national format: (809) 555-0199
    return `(${areaCode}) ${prefix}-${line}`;
  }
}
