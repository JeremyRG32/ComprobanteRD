import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-show-error',
  imports: [],
  templateUrl: './show-error.html',
  styleUrl: './show-error.css',
})
export class ShowError {
  @Input()
  error!: string;

  @Input()
  errors!: string[];
}
