import { observable, computed, action } from "mobx";
import { IUser, IUserFormValues } from "../models/user";
import agent from "../api/agent";

export default class UserStore {
  @observable user: IUser | null = null;

  @computed get isLoggedIn() {return !!this.user}

  @action login = async (values: IUserFormValues) => {
    try {
      const user = await agent.User.login(values);
      this.user = user;
    } catch (error) {
      console.log(error)
    }
  }
}