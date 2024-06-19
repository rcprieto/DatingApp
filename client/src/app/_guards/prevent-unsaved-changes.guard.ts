import { CanDeactivateFn } from '@angular/router';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';

export const preventUnsavedChangesGuard: CanDeactivateFn<MemberEditComponent> = (component) => {
  if(component.editForm?.dirty)
    {
      return confirm('Tem certeza eu quer sair da páginas? Dados quem foram alterados serão perdidos');
    }

  return true;
};
