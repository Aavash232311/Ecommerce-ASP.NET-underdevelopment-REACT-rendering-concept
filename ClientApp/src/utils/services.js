import jwt_decode from "jwt-decode";

export class Services {
  getServer() {
    return "https://localhost:7142";
  }
  getClientDomain() {
    return "https://localhost:44466";
  }
  token() {
    return localStorage.getItem("authToken");
  }
  getAbsolutePath(loc) {
    const domain = this.getClientDomain();
    const domainLimit = domain.length;
    return loc.substring(
      domainLimit,
      loc.length
    );
  }

  isLoggedIn() {
    if (this.token() === null) {
      return Promise.resolve({ value: false });
    }
    return fetch(this.getServer() + "/auth/check/", {
      credentials: "include",
      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + this.token(),
      },
    })
      .then((rsp) => rsp.json())
      .then((response) => {
        return response;
      });
  }

  getUserRole() {
    const token = localStorage.getItem("authToken");
    if (token == null) return null;
    const decoded = jwt_decode(token);
    for (const key in decoded) {
      if (
        key ===
        "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
      ) {
        const userRole = decoded[key];
        return userRole;
      }
    }
  }

}
