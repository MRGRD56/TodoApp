import { TestBed } from '@angular/core/testing';

import { TodoHubService } from './todo-hub.service';

describe('TodoHubService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: TodoHubService = TestBed.get(TodoHubService);
    expect(service).toBeTruthy();
  });
});
