import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';

import { TeamService } from './team';

describe('TeamService', () => {
  let service: TeamService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(TeamService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should POST to /api/teams when creating a team', () => {
    service.create('Kanto Squad').subscribe();

    const req = httpMock.expectOne('/api/teams');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ name: 'Kanto Squad' });
    req.flush({ id: 1, name: 'Kanto Squad', createdDate: '', members: [] });
  });

  it('should POST the pokemon id when adding a team member', () => {
    service.addPokemon(1, 25).subscribe();

    const req = httpMock.expectOne('/api/teams/1/pokemon');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ pokemonId: 25 });
    req.flush({ id: 1, name: 'Kanto Squad', createdDate: '', members: [] });
  });

  it('should DELETE the member slot when removing a team member', () => {
    service.removePokemon(1, 7).subscribe();

    const req = httpMock.expectOne('/api/teams/1/pokemon/7');
    expect(req.request.method).toBe('DELETE');
    req.flush({ id: 1, name: 'Kanto Squad', createdDate: '', members: [] });
  });
});
