export function getErrors(obj: any): string[] {
  const body = obj?.error;
  const err = body?.errors;

  // Validation errors: { errors: { Field: ["msg1", "msg2"] } }
  if (err) {
    let errorMessage: string[] = [];
    for (let key in err) {
      const messageField = err[key].map((message: string) => `${key}: ${message}`);
      errorMessage = errorMessage.concat(messageField);
    }
    return errorMessage;
  }

  // Business rule errors: { message: "..." }
  if (body?.message) {
    return [body.message];
  }

  // Plain string body
  if (typeof body === 'string') {
    return [body];
  }

  return ['Ocurrió un error inesperado.'];
}

export function getIdentityErrors(obj: any): string {
  let errorMessage: string = obj.error;
  return errorMessage;
}
