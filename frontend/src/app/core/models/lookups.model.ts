export interface LookupItem {
  id: number;
  name: string;
}

export interface Lookups {
  academicYears: LookupItem[];
  subjects: LookupItem[];
}
