import {
    HttpErrorResponse
} from '@angular/common/http';


export function getApiErrorMessage(
    error: unknown,
    fallbackMessage: string
): string {

    const httpError =
        error as HttpErrorResponse;


    // Backend kapalı, bağlantı reddedildi,
    // CORS/ağ problemi vb.
    if (httpError?.status === 0) {

        return 'Sunucuya ulaşılamıyor. Backend servisinin çalıştığını kontrol ediniz.';
    }


    const responseBody =
        httpError?.error as {
            message?: unknown;
            errors?: Record<
                string,
                unknown
            >;
        } | null;


    // Backend tarafından gönderilen
    // özel mesaj varsa öncelik ver.
    if (
        typeof responseBody?.message ===
        'string' &&
        responseBody.message.trim()
    ) {

        return responseBody.message;
    }


    // ASP.NET Core validation
    // ProblemDetails hatalarını yakala.
    if (
        responseBody?.errors &&
        typeof responseBody.errors ===
        'object'
    ) {

        for (
            const value of Object.values(
                responseBody.errors
            )
        ) {

            if (
                Array.isArray(value) &&
                value.length > 0 &&
                typeof value[0] ===
                'string'
            ) {

                return value[0];
            }


            if (
                typeof value ===
                'string' &&
                value.trim()
            ) {

                return value;
            }
        }
    }


    if (httpError?.status === 401) {

        return 'Oturumunuz geçersiz veya süresi dolmuş.';
    }


    if (httpError?.status === 403) {

        return 'Bu işlem için gerekli yetkiye sahip değilsiniz.';
    }


    if (httpError?.status === 404) {

        return 'İstenen kayıt bulunamadı.';
    }


    if (httpError?.status === 409) {

        return 'İşlem mevcut başka bir kayıtla çakışıyor.';
    }


    if (
        typeof httpError?.status ===
        'number' &&
        httpError.status >= 500
    ) {

        return 'Sunucu tarafında beklenmeyen bir hata oluştu.';
    }


    // Service içerisinde bizim fırlattığımız
    // normal Error nesneleri için.
    if (
        error instanceof Error &&
        error.message
    ) {

        return error.message;
    }


    return fallbackMessage;
}