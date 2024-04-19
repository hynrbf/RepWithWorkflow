import RestBase from "../RestBase";
import { container, singleton } from "tsyringe";
import axios from "axios";
import { AUTH0_TOKEN_URL, REMOTE_API, SIGNUP_USER } from "@/config";
import { IAuth0Service } from "./IAuth0Service";
import jwt_decode from "jwt-decode";
import { AppConstants } from "@/infra/AppConstants";
import { AuthUser } from "@/entities/AuthUser";
import {
  IAppService,
  IAppServiceInfo,
} from "@/infra/dependency-services/app/IAppService";
import { OnboardingType } from "@/infra/base";
import { AuthAccess } from "@/entities/AuthAccess";

@singleton()
export default class Auth0Service extends RestBase implements IAuth0Service {
  appService: IAppService = container.resolve<IAppService>(
    IAppServiceInfo.name,
  );

  public async AuthenticateAsync(
    accessToken: string,
    idToken: string,
  ): Promise<string> {
    try {
      const payload = JSON.stringify({
        id_token: idToken,
        access_token: accessToken,
      });
      const response = await axios.post(AUTH0_TOKEN_URL, payload);
      return response.data["authenticationToken"]?.toString();
    } catch (error) {
      throw new Error(`Auth error: ${error}`);
    }
  }

  public async getAccessTokenForSignUpAsync(): Promise<{
    access_token: string;
    id_token: string;
  }> {
    try {
      return await this.postRemoteAsync<AuthAccess>(
        `${REMOTE_API}/GetAccessTokenForSignupAsync`,
        "",
      );
    } catch (error) {
      throw new Error(`Auth error: ${error}`);
    }
  }

  public async getTokenForSignupAndSaveLocallyAsync(): Promise<boolean> {
    const token = await this.getAccessTokenForSignUpAsync();

    if (!token) {
      return false;
    }

    const authToken = await this.AuthenticateAsync(
      token.access_token,
      token.id_token,
    );

    if (!authToken) {
      return false;
    }

    const decodedTokenDetails = jwt_decode(token.id_token) as { exp: number };
    const oneHourAdvanceExpiry = decodedTokenDetails.exp - 3600;
    localStorage.setItem(AppConstants.authTokenCacheKey, authToken);

    const authUser: AuthUser = {
      token: authToken,
      tokenExpiry: oneHourAdvanceExpiry,
      isAuthenticated: true,
      email: SIGNUP_USER,
    };
    this.appService.saveAuthUserToLocal(authUser);
    return true;
  }

  public async changePasswordAsync(
    email: string,
    newPassword: string,
    onboardingType: string = OnboardingType.Firm.toString(),
  ): Promise<boolean> {
    try {
      const payload = JSON.stringify({
        email: email,
        password: newPassword,
        onboardingType: onboardingType,
      });
      return await this.postRemoteAsync<boolean>(
        `${REMOTE_API}/ChangePasswordAsync`,
        payload,
      );
    } catch (error) {
      throw new Error(`Auth error: ${error}`);
    }
  }
}
