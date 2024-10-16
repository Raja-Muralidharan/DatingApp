export interface Pagination{
    currentPage: number;
    itemsPerPage: number;
    totalItems: number;
    toalPages: number;
}

export class PaginatedResult<T> {
    items?: T;
    pagination?: Pagination
}