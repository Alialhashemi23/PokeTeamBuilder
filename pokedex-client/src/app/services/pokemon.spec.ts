import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';

import { Pokemon } from './pokemon';

describe('Pokemon', () => {
  let service: Pokemon;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(Pokemon);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
