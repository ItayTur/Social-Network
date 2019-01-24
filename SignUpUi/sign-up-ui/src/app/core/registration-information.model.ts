export class RegistrationInfoModel {
    password:string;
    email: string;
    firstName: string;
    lastName: string;
    address: string;
    job: string;
    birthDate: Date;

    constructor(password:string, email: string, firstName: string, lastName: string, address: string, job: string, birthDate:Date) {
      this.password = password;
      this.email = email;
      this.firstName = firstName;
      this.lastName = lastName;
      this.address = address;
      this.job = job;
      this.birthDate = birthDate;
    }
}
