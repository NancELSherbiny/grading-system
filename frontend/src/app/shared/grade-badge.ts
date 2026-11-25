import { Component, input } from '@angular/core';

/** A coloured letter grade, or a dash when there's no grade yet. */
@Component({
  selector: 'app-grade-badge',
  template: `
    @if (grade(); as grade) {
      <span class="badge grade-{{ grade }}">{{ grade }}</span>
    } @else {
      <span class="none">—</span>
    }
  `,
  styles: `
    .badge {
      display: inline-block;
      min-width: 28px;
      padding: 2px 8px;
      border-radius: 6px;
      font-weight: 700;
      text-align: center;
    }
    .grade-A { color: #1b5e20; background: #dcf3e4; }
    .grade-B { color: #0b5f73; background: #d8f0f5; }
    .grade-C { color: #6b5200; background: #fbefc6; }
    .grade-D { color: #8a4300; background: #fde3cc; }
    .grade-F { color: #a1121b; background: #fbdcdd; }
    .none { color: #9ca3af; }
  `,
})
export class GradeBadge {
  readonly grade = input<string | null>(null);
}
