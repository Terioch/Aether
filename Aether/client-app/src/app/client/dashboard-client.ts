import { DashboardView } from "../models/dashboard-view";
import { DashboardViewRequest } from "../requests/dashboard-view-request";

export default class DashboardClient {
  async getDashboardView(
    request: DashboardViewRequest,
  ): Promise<DashboardView> {
    return new Promise(async (resolve, reject) => {
      try {
        const res = await fetch(
          `${process.env.NEXT_PUBLIC_API_URL}/api/dashboard`,
          {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
            },
            body: JSON.stringify(request),
          },
        );

        if (!res.ok) {
          throw new Error("Failed to fetch pollution data");
        }

        const data = await res.json();
        resolve(data);
      } catch (error) {
        reject(error);
      }
    });
  }
}
