import {v4 as uuid } from "uuid";

export class Director {
  public id: string = uuid();
  public title: string = "";
  public forename: string = "";
  public surname: string = "";
  public dateOfBirth: number = 0;
  public position: string = "";
  public cvUrlLink: string = "";
}