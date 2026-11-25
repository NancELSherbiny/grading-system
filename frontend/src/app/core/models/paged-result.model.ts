/** One page of a list, as returned by the API. */
export interface PagedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export type SortDirection = 'asc' | 'desc';
