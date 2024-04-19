export type ActionFunction = () => void;

interface Button {
  text: string;
  classes?: string;
  action?: ActionFunction;
}

interface CancelIcon {
  enabled: boolean;
}

interface AttachTo {
  element: string;
  on: string; // This could be further refined to a union of specific strings if there are only certain valid positions.
}

export interface OnBoardingStep {
  id: string;
  classes: string;
  attachTo?: AttachTo;
  text: () => string;
  buttons: Button[];
  cancelIcon: CancelIcon;
  arrow: boolean;
}
