import { singleton } from "tsyringe";
import { IStationeryService } from "./IStationeryService";
import { StationeryEntity } from "@/entities/stationery/StationeryEntity";
import { v4 as uuid } from "uuid";
import moment from "moment";
import { StationeryStatus } from "@/entities/stationery/StationeryStatus";

@singleton()
export default class StationeryMockService implements IStationeryService {
  private _stationeries: StationeryEntity[] = [
    {
      id: uuid(),
      name: "Logo",
      icon: "clipboard-text-22",
      files: [],
      status: StationeryStatus.Approved,
      createdAt: moment().subtract(5, "d").unix(),
      updatedAt: moment().subtract(6, "d").unix(),
    },
    {
      id: uuid(),
      name: "Business Card",
      icon: "clipboard-text-22",
      files: [],
      status: StationeryStatus.Approved,
      createdAt: moment().subtract(15, "d").unix(),
      updatedAt: moment().subtract(16, "d").unix(),
    },
    {
      id: uuid(),
      name: "Letterhead",
      icon: "clipboard-text-22",
      files: [],
      status: StationeryStatus.Approved,
      createdAt: moment().subtract(25, "d").unix(),
      updatedAt: moment().subtract(6, "d").unix(),
    },
    {
      id: uuid(),
      name: "Compliment Slip",
      icon: "clipboard-text-22",
      files: [],
      status: StationeryStatus.Approved,
      createdAt: moment().subtract(35, "d").unix(),
      updatedAt: moment().subtract(16, "d").unix(),
    },
    {
      id: uuid(),
      name: "Email Footer",
      icon: "clipboard-text-22",
      files: [],
      status: StationeryStatus.Approved,
      createdAt: moment().subtract(35, "d").unix(),
      updatedAt: moment().unix(),
    },
  ];

  public getStationeriesAsync(): Promise<StationeryEntity[]> {
    return Promise.resolve(this._stationeries);
  }

  public getStationeryAsync(id: string): Promise<StationeryEntity | undefined> {
    return Promise.resolve(this._stationeries.find((item) => item.id === id));
  }

  public addStationeryAsync(
    payload: Partial<StationeryEntity>,
  ): Promise<StationeryEntity> {
    const newItem : StationeryEntity = {
      ...payload,
      id: uuid(),
      status: StationeryStatus.Pending,
      createdAt: moment().unix(),
      updatedAt: moment().unix(),

    };

    this._stationeries = [...this._stationeries, newItem];
    return Promise.resolve(newItem);
  }

  public updateStationeryAsync(
    id: string,
    payload: Partial<StationeryEntity>,
  ): Promise<StationeryEntity> {
    let updatedItem: StationeryEntity =
        {id: undefined};
    this._stationeries = this._stationeries.map((item) => {
      if (item.id === id) {
        return (updatedItem = {
          ...item,
          ...payload,
          updatedAt: moment().unix(),
        });
      }

      return item;
    });
    return Promise.resolve(updatedItem);
  }
}
