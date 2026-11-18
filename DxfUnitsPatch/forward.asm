EXTERN EnsureOriginal:PROC
EXTERN MaybeRewrite:PROC
EXTERN g_LoadKompasLibrary:QWORD
EXTERN g_UnloadKompasLibrary:QWORD
EXTERN g_writeTrampoline:QWORD

.code

; Штатные вызовы передают аргументы в rcx/rdx/r8/r9.
; Сохраняем их, поднимаем оригинальную библиотеку и прыгаем в неё.
CallOriginal MACRO target
    push rcx
    push rdx
    push r8
    push r9
    sub rsp, 68h
    movaps [rsp+20h], xmm0
    movaps [rsp+30h], xmm1
    movaps [rsp+40h], xmm2
    movaps [rsp+50h], xmm3
    call EnsureOriginal
    movaps xmm3, [rsp+50h]
    movaps xmm2, [rsp+40h]
    movaps xmm1, [rsp+30h]
    movaps xmm0, [rsp+20h]
    add rsp, 68h
    pop r9
    pop r8
    pop rdx
    pop rcx
    jmp QWORD PTR [target]
ENDM

LoadKompasLibrary PROC
    CallOriginal g_LoadKompasLibrary
LoadKompasLibrary ENDP

UnloadKompasLibrary PROC
    CallOriginal g_UnloadKompasLibrary
UnloadKompasLibrary ENDP

; Вход как у WriteFile. Подменяет буфер и прыгает в хвост KernelBase!WriteFile.
HookWriteFile PROC
    push rbx
    push rsi
    push rdi
    push r12
    mov rbx, rcx
    mov rsi, rdx
    mov edi, r8d
    mov r12, r9
    sub rsp, 28h
    mov rcx, rsi
    mov edx, edi
    call MaybeRewrite
    add rsp, 28h
    mov rcx, rbx
    mov rdx, rsi
    mov r8d, edi
    mov r9, r12
    test rax, rax
    jz write_go
    mov rdx, rax
write_go:
    pop r12
    pop rdi
    pop rsi
    pop rbx
    jmp QWORD PTR [g_writeTrampoline]
HookWriteFile ENDP

END
