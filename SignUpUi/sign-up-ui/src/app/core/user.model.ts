export class UserModel {
    id:string;
    email: string;
    firstName: string;
    lastName: string;
    address: string;
    job: string;
    birthDate: Date;

    constructor(id:string, email: string, firstName: string, lastName: string, address: string, job: string, birthDate:Date) {
      this.id = id;
      this.email = email;
      this.firstName = firstName;
      this.lastName = lastName;
      this.address = address;
      this.job = job;
      this.birthDate = birthDate;
    }
  }
