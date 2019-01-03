import { TestBed } from '@angular/core/testing';

import { FacebookLoginService } from './facebook-login.service';

describe('FacebookLoginService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: FacebookLoginService = TestBed.get(FacebookLoginService);
    expect(service).toBeTruthy();
  });
});
