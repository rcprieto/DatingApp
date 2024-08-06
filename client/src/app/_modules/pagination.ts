export interface Pagination{
  currentPage: number;
  itemsPerPage: number;
  totalItems: number;
  totalPages: number;
}

export class PaginatedResut<T>{
  result?: T;
  pagination?: Pagination;

}
