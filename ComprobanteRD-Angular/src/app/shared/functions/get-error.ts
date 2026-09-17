export function getErrors(obj: any): string[] {
  const err = obj.error.errors;

  let errorMessage: string[] = [];

  for (let key in err) {
    let field = key;
    const messageField = err[key].map((mensaje: string) => `${field}: ${mensaje}`);
    errorMessage = errorMessage.concat(messageField);
  }

  return errorMessage;
}

export function getIdentityErrors(obj: any): string {
  let errorMessage: string = obj.error;
  return errorMessage;
}
