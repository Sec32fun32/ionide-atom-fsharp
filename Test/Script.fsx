module XA =

  let funky x = x + 1
  let foo = 1

module YA =
  let bar = XA.funky

let foobar = YA.bar XA.foo

let direct = System.IO.Directory
