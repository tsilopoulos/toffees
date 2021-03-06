import { Injectable, Inject } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { IGlucose, Glucose } from "../glucose/glucose.component";

@Injectable()
export class GlucoseService {

  apiGatewayUrl: string;

  constructor(private readonly httpClient: HttpClient, @Inject("API_GATEWAY_URL") apiGatewayUrl: string) {
    this.apiGatewayUrl = apiGatewayUrl;
  }

  post(bg: Glucose) {
    return this.httpClient
    .post(this.apiGatewayUrl + `api/glucose/${localStorage.getItem("userId")}`, bg)
    .do((response: IGlucose) => {
      try {
        if (response) {
          // We probably don't have to do something here
        }
      } catch (e) {
        // TODO add error handling
      }
    }, error => console.error(error));
  }

  put(bg: Glucose) {
    return this.httpClient
    .put(this.apiGatewayUrl + `api/glucose/${localStorage.getItem("userId")}`, bg)
    .do((response: IGlucose) => {
      try {
        if (response) {
          // We probably don't have to do something here
        }
      } catch (e) {
        // TODO add error handling
      }
    }, error => console.error(error));
  }

  delete(bgId: number) {
    return this.httpClient
      .delete(this.apiGatewayUrl + `api/glucose/${bgId}`)
      .do((response: any) => {
        try {
          if (response) {
            // We probably don't have to do something here either
          }
        } catch (e) {
          // TODO add error handling
        }
      }, error => console.error(error));
  }
}
