import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, shareReplay } from 'rxjs';
import { Lookups } from '../models/lookups.model';

@Injectable({ providedIn: 'root' })
export class LookupsService {
  private readonly http = inject(HttpClient);

  // Academic years and subjects never change, so they're loaded once and shared.
  private readonly lookups$ = this.http.get<Lookups>('/api/lookups').pipe(shareReplay(1));

  getLookups(): Observable<Lookups> {
    return this.lookups$;
  }
}
