export interface LoginDTO {
  email: string;
  password: string;
}

export interface AuthResponseDTO {
  token: string;
  expiration: Date;
  companyId: number;
  Roles: string[];
}
