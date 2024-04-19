export class CategoryEntity {
  public name: string | undefined;
  public displayText: string | undefined;
  public products: ProductEntity[] = [];
}

export class ProductEntity {
  public name: string | undefined;
  public displayText: string | undefined;
}
