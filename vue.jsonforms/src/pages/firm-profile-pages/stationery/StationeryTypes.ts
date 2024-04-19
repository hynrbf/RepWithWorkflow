export interface DisclosureArrayItem {
    id: string;
    title: string;
    initialItems?: string[]; 
    emptyText: string;
    isFeatured?: boolean;
    noAdd?: boolean;
    keepEditting?: boolean;
  }