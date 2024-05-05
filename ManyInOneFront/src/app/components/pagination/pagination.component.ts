import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-pagination',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './pagination.component.html',
  styles: ``
})
export class PaginationComponent implements OnInit {
  @Input() totalItems: number = 0;
  @Input() currentPage: number = 1;
  @Input() itemsPerPage: number = 5;
  @Output() onClick : EventEmitter<number> = new EventEmitter();

  totalPages: number = 0;
  pages : number[] = []

  constructor() {

  }
  ngOnInit(): void {
    if (this.totalItems) {
      this.totalPages = Math.ceil(this.totalItems / this.itemsPerPage);
      // alert(this.totalPages);
      this.pages = Array.from({length : this.totalPages}, (_, i) => i + 1);
    }
  }

  pageClicked(page : number)
  {
    if(page <= this.totalPages)
    {
      this.onClick.emit(page);
    }
  }
}
