namespace Factograph2.Core

(* 
    ATOMS AND VARIABLES

    Atoms are just identifiers in Factograph, basically strings which you can only check for equality.
    Since atom and variable names are not required for any computation (only their identities), 
    we can later change them to ints for better performance.
    That's why we're making them their own types instead of just aliases.

    Examples of atoms: home, bob, cat.
    Examples of variables: X, Y, World.
*)
type AtomId = AtomId of string
type VarId  = VarId of string

(* 
    VARIANTS

    Variant is simply a value of any of several basic types.

    Examples of variants: 42, 3.14, "hello world".
*)
type Variant =
    | Integer of i : int
    | String  of s : string
    | Float   of f : double

(*
    TERMS

    Terms are basic building blocks of Factograph, similar to statements or function calls in other languages.
    Terms can be simple or complex, creating a tree-like expression structure.

    Examples of terms: 42, bob, X, age(bob, 42), age(bob, X), square(origin(X, 10), size(20, Y)).
*)
type Term = 
    | Atom     of id : AtomId
    | Constant of value : Variant
    | Variable of name : VarId
    | Complex  of Term list
