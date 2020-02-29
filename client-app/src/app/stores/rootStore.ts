import ActivityStore from './activityStore';
import UserStore from './userStore';
import { createContext } from 'react';

export class RootStore {
  activityStore: ActivityStore;
  userStore: UserStore;

  constructor() {
    this.activityStore = new ActivityStore(this);
    this.userStore = new UserStore(this);
  }
}

export const RootStoreContext = createContext(new RootStore());