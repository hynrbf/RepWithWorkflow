import { RouteProps } from "@/entities/owners-and-controllers/RouteProps";

export declare interface IPagesRouteService {
  registerRoutes(data: RouteProps[]): void;

  getCurrentRoutes(): RouteProps[];

  getCurrentRoutePaths(): string[];

  getSequenceNo(routeName: string): number;
}