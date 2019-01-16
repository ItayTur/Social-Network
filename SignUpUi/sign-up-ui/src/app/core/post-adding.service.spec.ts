import { TestBed } from '@angular/core/testing';

import { PostAddingService } from './post-adding.service';

describe('PostAddingService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: PostAddingService = TestBed.get(PostAddingService);
    expect(service).toBeTruthy();
  });
});
