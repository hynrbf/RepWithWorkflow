export declare interface IAuth0Service {
  AuthenticateAsync(accessToken: string, idToken: string): Promise<string>;

  getAccessTokenForSignUpAsync(): Promise<{
    access_token: string;
    id_token: string;
  }>;

  getTokenForSignupAndSaveLocallyAsync(): Promise<boolean>;

  changePasswordAsync(
    email: string,
    newPassword: string,
    onboardingType: string,
  ): Promise<boolean>;
}

export const IAuth0ServiceInfo = {
  name: "IAuth0Service",
};