namespace Factograph2.Core

(* 
    ATOMS AND VARIABLES

    Atoms are just identifiers in Factograph, basically strings which you can only check for equality.
    Since atom and variable names are not required for any computation (only their identities), 
    we can later change them to ints for better performance.

    Examples of atoms: home, bob, cat.
    Examples of variables: X, Y, World.
*)
type AtomId = string
type VarId  = string

module IdEx =
    let AtomFromString str = str : AtomId
    let VarFromString str = str : VarId

(* 
    VARIANTS

    Variant is simply a value of any of several basic types - or just any .NET object.

    Examples of variants: 42, 3.14, "hello world".
*)
type Variant =
    | Integer of i : int
    | String  of s : string
    | Bool    of b : bool
    | Float   of f : double
    | Object  of o : obj

    static member ToVariant (o : obj) = 
        match o with
            | :? int    as i -> Variant.Integer i
            | :? string as s -> Variant.String s
            | :? bool   as b -> Variant.Bool b
            | :? float  as f -> Variant.Float f
            | _ -> Variant.Object o

(*
    TERMS

    Terms are basic building blocks of Factograph, similar to statements or function calls in other languages.
    Values of all variables are terms.
    Terms can be simple or complex, creating a tree-like expression structure.

    Examples of terms: 42, bob, X, age(bob, 42), age(bob, X), square(origin(X, 10), size(20, Y)).
*)
type Term = 
    | Atom     of id : AtomId
    | Constant of value : Variant
    | Variable of name : VarId
    | Complex  of Term list

    member self.IsPredicate = 
        match self with
            | Complex (Atom _ :: _) -> true
            | _ -> false
        