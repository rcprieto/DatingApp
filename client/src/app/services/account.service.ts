import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { User } from '../_modules/user';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  baseUrl = 'https://localhost:5001/api/'

  private currentUserSource = new BehaviorSubject<User | null>(null);
  //$ é para dizer que é um observable
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient) { }

   login(model: any)
   {
    //Faz um post na API, usa o pipe para tratar o dado antes do subscribe, map transforma o retorno em um objeto User, coloca no localStorage e seta o currentUserSource com o user
      return this.http.post<User>(this.baseUrl + 'account/login', model)
      .pipe(map((response: User) => {
          const user = response;
          if(user){
            localStorage.setItem('user', JSON.stringify(user));
            this.setCurrentUser(user);

          }

        })

      );

   }

   register(model: any)
   {
      return this.http.post<User>(this.baseUrl + 'account/register', model).pipe(
        map(user => {
          if(user)
            {
              localStorage.setItem('user', JSON.stringify(user));
              this.currentUserSource.next(user);
            }
            return user;
        })
      )
   }

   logout(){
    localStorage.removeItem('user');
    this.setCurrentUser(null);
   }

   setCurrentUser(user: User | null){
    this.currentUserSource.next(user);
   }
}