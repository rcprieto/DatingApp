import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-test-error',
  templateUrl: './test-error.component.html',
  styleUrls: ['./test-error.component.css']
})
export class TestErrorComponent implements OnInit {
  baseUrl = 'https://localhost:5001/api/'
  validationError: string[] = [];
  constructor(private http: HttpClient) {

  }

  ngOnInit(): void {

  }

  get404Erro(){
    this.http.get(this.baseUrl + "buggy/not-found").subscribe({
      next: resp => console.log(resp),
      error: er => console.log(er)

    });
  }

  get400Error(){
    this.http.get(this.baseUrl + "buggy/bad-request").subscribe({
      next: resp => console.log(resp),
      error: er => console.log(er)

    });
  }

  get500Error(){
    this.http.get(this.baseUrl + "buggy/server-error").subscribe({
      next: resp => console.log(resp),
      error: er => console.log(er)

    });
  }

  get401Error(){
    this.http.get(this.baseUrl + "buggy/auth").subscribe({
      next: resp => console.log(resp),
      error: er => console.log(er)

    });
  }

  get400ValidationError(){
    this.http.post(this.baseUrl + "account/register", {}).subscribe({
      next: resp => console.log(resp),
      error: er => {
        this.validationError = er;
      }

    });
  }

}
