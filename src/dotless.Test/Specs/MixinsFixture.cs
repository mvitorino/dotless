namespace dotless.Test.Specs
{
    using System;
    using System.Globalization;
    using System.Threading;
    using NUnit.Framework;

    public class MixinsFixture : SpecFixtureBase
    {
        [Test]
        public void Mixins()
        {
            // Todo: split into separate atomic tests.
            var input =
                @"
.mixin { border: 1px solid black; }
.mixout { border-color: orange; }
.borders { border-style: dashed; }

#namespace {
  .borders {
    border-style: dotted;
  }
  .biohazard {
    content: ""death"";
    .man {
      color: transparent;
    }
  }
}
#theme {
  > .mixin {
    background-color: grey;
  }
}
#container {
  color: black;
  .mixin;
  .mixout;
  #theme > .mixin;
}

#header {
  .milk {
    color: white;
    .mixin;
    #theme > .mixin;
  }
  #cookie {
    .chips {
      #namespace .borders;
      .calories {
        #container;
      }
    }
    .borders;
  }
}
.secure-zone { #namespace .biohazard .man; }
.direct {
  #namespace > .borders;
}

";

            var expected =
                @"
.mixin {
  border: 1px solid black;
}
.mixout {
  border-color: orange;
}
.borders {
  border-style: dashed;
}
#namespace .borders {
  border-style: dotted;
}
#namespace .biohazard {
  content: ""death"";
}
#namespace .biohazard .man {
  color: transparent;
}
#theme > .mixin {
  background-color: grey;
}
#container {
  color: black;
  border: 1px solid black;
  border-color: orange;
  background-color: grey;
}
#header .milk {
  color: white;
  border: 1px solid black;
  background-color: grey;
}
#header #cookie {
  border-style: dashed;
}
#header #cookie .chips {
  border-style: dotted;
}
#header #cookie .chips .calories {
  color: black;
  border: 1px solid black;
  border-color: orange;
  background-color: grey;
}
.secure-zone {
  color: transparent;
}
.direct {
  border-style: dotted;
}
";

            AssertLess(input, expected);
        }

        [Test, Ignore("Unsupported")]
        public void CommaSeparatedMixins()
        {
            // Note: http://github.com/cloudhead/less.js/issues/issue/8

            var input =
                @"
.mixina() {
  color: red;
}
.mixinb() {
  color: green;
}

.class {
  .mixina, .mixinb;
}
";

            var expected = @"
.class {
  color: red;
  color: green;
}
";

            AssertLess(input, expected);
        }

        [Test]
        public void ChildSelector()
        {
            var input =
                @"
#bundle {
  .mixin {
    padding: 20px;
    color: purple;
  }
}

#header {
  #bundle > .mixin;
}
";

            var expected =
                @"
#bundle .mixin {
  padding: 20px;
  color: purple;
}
#header {
  padding: 20px;
  color: purple;
}
";

            AssertLess(input, expected);
        }

        [Test]
        public void MixinNestedRules()
        {
            var input =
                @"
.bundle() {
  p {
    padding: 20px;
    color: purple;
    a { margin: 0; }
  }
}

#header {
  .bundle;
}
";

            var expected = @"
#header p {
  padding: 20px;
  color: purple;
}
#header p a {
  margin: 0;
}
";

            AssertLess(input, expected);
        }

        [Test]
        public void MultipleMixins()
        {
            var input = @"
.mixin{
    border:solid 1px red;
}
.mixin{
    color:blue;
}
.mix-me-in{
    .mixin;
}
";

            var expected =
                @"
.mixin {
  border: solid 1px red;
}
.mixin {
  color: blue;
}
.mix-me-in {
  border: solid 1px red;
  color: blue;
}
";

            AssertLess(input, expected);
        }


        [Test]
        public void MixinWithArgs()
        {
            var input =
                @".mixin (@a: 1px, @b: 50%) {
  width: @a * 5;
  height: @b - 1%;
}
 
.mixin-arg {
  .mixin(4px, 21%);
}";

            var expected =
                @".mixin-arg {
  width: 20px;
  height: 20%;
}";

            AssertLess(input, expected);
        }

        [Test, Ignore("Unsupported")]
        public void CanPassNamedArguments()
        {
            var input =
                @".mixin (@a: 1px, @b: 50%) {
  width: @a * 5;
  height: @b - 1%;
}
 
.named-arg {
  color: blue;
  .mixin(@b: 100%);
}";

            var expected =
                @".named-arg {
  color: blue;
  width: 5px;
  height: 99%;
}";

            AssertLess(input, expected);
        }

        [Test]
        public void CanPassVariablesAsPositionalArgs()
        {
            var input =
                @".mixin (@a: 1px, @b: 50%) {
  width: @a * 5;
  height: @b - 1%;
}
 
.class {
  @var: 20px;
  .mixin(@var);
}";

            var expected =
                @".class {
  width: 100px;
  height: 49%;
}";

            AssertLess(input, expected);
        }

        [Test, Ignore("Unsupported")]
        public void CanPassVariablesAsNamedArgs()
        {
            var input =
                @".mixin (@a: 1px, @b: 50%) {
  width: @a * 5;
  height: @b - 1%;
}
 
.class {
  @var: 20%;
  .mixin(@b: @var);
}";

            var expected =
                @".class {
  width: 5px;
  height: 19%;
}";

            AssertLess(input, expected);
        }

        [Test, Ignore("Unsupported")]
        public void MixedPositionalAndNamedArguments()
        {
            var input =
                @".mixin (@a: 1px, @b: 50%, @c: 50) {
  width: @a * 5;
  height: @b - 1%;
  color: #000000 + @c;
}
 
.mixed-args {
  .mixin(3px, @c: 100);
}";

            var expected =
                @".mixed-args {
  width: 15px;
  height: 49%;
  color: #646464;
}";

            AssertLess(input, expected);
        }

        [Test, Ignore("Unsupported")]
        public void PositionalArgumentsMustAppearBeforeAllNamedArguments()
        {
            var input =
                @".mixin (@a: 1px, @b: 50%, @c: 50) {
  width: @a * 5;
  height: @b - 1%;
  color: #000000 + @c;
}
 
.oops {
  .mixin(@c: 100, 3px);
}";

            AssertError("Positional arguments must appear before all named arguments. in '.mixin(@c: 100, 3px)'", input);
        }

        [Test, Ignore("Unsupported")]
        public void ThrowsIfArumentNotFound()
        {
            var input =
                @".mixin (@a: 1px, @b: 50%) {
  width: @a * 3;
  height: @b - 1%;
}
 
.override-inner-var {
  .mixin(@var: 6);
}";

            AssertError("Argument '@var' not found. in '.mixin(@var: 6)'", input);
        }

        [Test]
        public void ThrowsIfTooManyArguments()
        {
            var input =
                @"
.mixin (@a: 5) {  width: @a * 5; }

.class { .mixin(1, 2, 3); }";

            AssertError(
                "No matching definition was found for `.mixin(1, 2, 3)`",
                ".class { .mixin(1, 2, 3); }",
                3,
                9,
                input);
        }

        [Test]
        public void MixinWithArgsInsideNamespace()
        {
            var input =
                @"#namespace {
  .mixin (@a: 1px, @b: 50%) {
    width: @a * 5;
    height: @b - 1%;
  }
}

.namespace-mixin {
  #namespace .mixin(5px);
}";

            var expected =
                @".namespace-mixin {
  width: 25px;
  height: 49%;
}";

            AssertLess(input, expected);
        }

        [Test]
        public void NestedParameterizedMixins1()
        {
            var input =
                @"
.outer(@a: 5) {
  .inner (@b: 10) {
    width: @a + @b;
  }
}

.class {
  .outer;
}
";

            var expected = "";

            AssertLess(input, expected);
        }

        [Test]
        public void NestedParameterizedMixins2()
        {
            var input =
                @"
.outer(@a: 5) {
  .inner (@b: 10) {
    width: @a + @b;
  }
}

.class {
  .outer;
  .inner;
}
";

            var expected = @"
.class {
  width: 15;
}
";

            AssertLess(input, expected);
        }

        [Test, Ignore("Unsupported")]
        public void NestedParameterizedMixins3()
        {
            var input =
                @"
.outer(@a: 5) {
  .inner (@b: 10) {
    width: @a + @b;
  }
}

.class {
  .outer .inner;
}
";

            var expected = @"
.class {
  width: 15;
}
";

            AssertLess(input, expected);
        }

        [Test]
        public void NestedParameterizedMixins4()
        {
            var input =
                @"
.outer(@a: 5) {
  .inner (@b: 10) {
    width: @a + @b;
  }
}

.class {
  .outer(1);
  .inner(2);
}
";

            var expected = @"
.class {
  width: 3;
}
";

            AssertLess(input, expected);
        }

        [Test, Ignore("Unsupported")]
        public void NestedParameterizedMixins5()
        {
            var input =
                @"
.outer(@a: 5) {
  .inner (@b: 10) {
    width: @a + @b;
  }
}

.class {
  .outer(2) .inner(4);
}
";

            var expected = @"
.class {
  width: 6;
}
";

            AssertLess(input, expected);
        }

        [Test]
        public void NestedRulesInMixinsShouldRespectArguments()
        {
            var input =
                @"
.mixin(@a: 5) {
    .someClass {
        width: @a;
    }
}

.class1 { .mixin(1); }
.class2 { .mixin(2); }
";

            var expected = @"
.class1 .someClass {
  width: 1;
}
.class2 .someClass {
  width: 2;
}
";

            AssertLess(input, expected);
        }

        [Test]
        public void MultipleCallsToMixinsContainingMixinCalls()
        {
            var input =
                @"
.mixintest(@a :5px){
    height: @a;
    input{
        .mixintest2(@a);
    }
}

.mixintest2(@a : 10px){
    width: @a;
}

.test{
    .mixintest();
}

.test2{
    .mixintest(15px);
}";

            var expected =
                @"
.test {
  height: 5px;
}
.test input {
  width: 5px;
}
.test2 {
  height: 15px;
}
.test2 input {
  width: 15px;
}";

            AssertLess(input, expected);
        }


        [Test]
        public void CanUseVariablesAsDefaultArgumentValues()
        {
            var input =
                @"@var: 5px;

.mixin (@a: @var, @b: 50%) {
  width: @a * 5;
  height: @b - 1%;
}


.class {
  .mixin;
}";

            var expected =
                @".class {
  width: 25px;
  height: 49%;
}";

            AssertLess(input, expected);
        }

        [Test]
        public void ArgumentsOverridesVariableInSameScope()
        {
            var input =
                @"@a: 10px;

.mixin (@a: 5px, @b: 50%) {
  width: @a * 5;
  height: @b - 1%;
}


.class {
  .mixin;
}";

            var expected =
                @".class {
  width: 25px;
  height: 49%;
}";

            AssertLess(input, expected);
        }

        [Test, Ignore("Infinite Loop - breaks tester")]
        public void CanUseArgumentsWithSameNameAsVariable()
        {
            var input =
                @"@a: 5px;

.mixin (@a: @a, @b: 50%) {
  width: @a * 5;
  height: @b - 1%;
}


.class {
  .mixin;
}";

            var expected =
                @".class {
  width: 25px;
  height: 49%;
}";

            AssertLess(input, expected);
        }

        [Test]
        public void CanNestParameterizedMixins()
        {
            var input =
                @"
.inner(@size: 12px) {
  font-size: @size;
}

.outer(@width: 20px) {
  width: @width;
  .inner(10px);
}

.class {
 .outer(12px);
}";

            var expected = @"
.class {
  width: 12px;
  font-size: 10px;
}";

            AssertLess(input, expected);
        }

        [Test]
        public void CanNestParameterizedMixinsWithDefaults()
        {
            var input =
                @"
.inner(@size: 12px) {
  font-size: @size;
}

.outer(@width: 20px) {
  width: @width;
  .inner();
}

.class {
 .outer();
}";

            var expected = @"
.class {
  width: 20px;
  font-size: 12px;
}";

            AssertLess(input, expected);
        }


        [Test]
        public void CanNestParameterizedMixinsWithSameParameterNames()
        {
            var input =
                @"
.inner(@size: 12px) {
  font-size: @size;
}

.outer(@size: 20px) {
  width: @size;
  .inner(14px);
}

.class {
 .outer(16px);
}";

            var expected = @"
.class {
  width: 16px;
  font-size: 14px;
}";

            AssertLess(input, expected);
        }

        [Test]
        public void IncludesAllMatchedMixins1()
        {
            var input =
                @"
.mixin () { zero: 0; }
.mixin (@a: 1px) { one: 1; }
.mixin (@a) { one-req: 1; }
.mixin (@a: 1px, @b: 2px) { two: 2; }
.mixin (@a: 1px, @b: 2px, @c: 3px) { three: 3; }

.zero { .mixin(); }

.one { .mixin(1); }

.two { .mixin(1, 2); }

.three { .mixin(1, 2, 3); }
";

            var expected =
                @"
.zero {
  zero: 0;
  one: 1;
  two: 2;
  three: 3;
}
.one {
  one: 1;
  one-req: 1;
  two: 2;
  three: 3;
}
.two {
  two: 2;
  three: 3;
}
.three {
  three: 3;
}
";

            AssertLess(input, expected);
        }

        [Test]
        public void IncludesAllMatchedMixins2()
        {
            var input =
                @"
.mixout ('left') { left: 1; }

.mixout ('right') { right: 1; }

.left { .mixout('left'); }
.right { .mixout('right'); }
";

            var expected = @"
.left {
  left: 1;
}
.right {
  right: 1;
}
";

            AssertLess(input, expected);
        }

        [Test]
        public void ThrowsIfNoMatchFound()
        {
            var input =
                @"
.mixout ('left') { left: 1; }

.mixout ('right') { right: 1; }

.none { .mixout('top'); }
";

            AssertError(
                "No matching definition was found for `.mixout('top')`",
                ".none { .mixout('top'); }",
                5,
                8,
                input);
        }

        [Test]
        public void ThrowsIfNotDefined()
        {
            var input = ".none { .mixin(); }";

            AssertError(
                ".mixin is undefined",
                ".none { .mixin(); }",
                1,
                8,
                input);
        }

        [Test]
        public void CallSiteCorrectWhenMixinThrowsAnError()
        {
            var divideByZeroException = new DivideByZeroException();
            
            var input = @"
.mixin(@a: 5px) {
  width: 10px / @a;
}
.error {
  .mixin(0px);
}";

            AssertError(
                divideByZeroException.Message,
                "  width: 10px / @a;",
                2,
                14,
                "  .mixin(0px);",
                5,
                input);
        }

        [Test]
        public void IncludesAllMatchedMixins3()
        {
            var input =
                @"
.border (@side, @width) {
    color: black;
    .border-side(@side, @width);
}
.border-side (left, @w) {
    border-left: @w;
}
.border-side (right, @w) {
    border-right: @w;
}

.border-right {
    .border(right, 4px);    
}
.border-left {
    .border(left, 4px);    
}
";

            var expected =
                @"
.border-right {
  color: black;
  border-right: 4px;
}
.border-left {
  color: black;
  border-left: 4px;
}";

            AssertLess(input, expected);
        }

        [Test]
        public void InnerMixinEvaluatedCorrectly()
        {
            var input =
                @"
.inner-mixin(@width) {
    width: @width;
}
.mixin() {
    span {
        color: red;
        .inner-mixin(30px);
    }
}

#header {
    .mixin();
}";

            var expected = @"
#header span {
  color: red;
  width: 30px;
}";

            AssertLess(input, expected);
        }

        [Test]
        public void InnerMixinsFindInnerVariables()
        {
            var input =
                @"
.inner-mixin(@width) {
    width: @width;
}
.mixin() {
    span {
        @var: 20px;
        .inner-mixin(@var);
    }
}

#header {
    .mixin();
}";

            var expected = @"
#header span {
  width: 20px;
}";

            AssertLess(input, expected);
        }

        [Test]
        public void ThrowsIfMixinNotFound()
        {
            var input =
                @"
.class {
  .mixin();
}
";
            AssertError(".mixin is undefined", "  .mixin();", 2, 2, input);
        }

        [Test]
        public void DontCacheFunctions()
        {
            var input =
                @"
.margin(@t, @r) {
  margin: formatString(""{0} {1}"", @t, @r);
}
ul.bla {
  .margin(10px, 15px);
}
ul.bla2 {
  .margin(0, 0);
}";

            var expected = @"
ul.bla {
  margin: 10px 15px;
}
ul.bla2 {
  margin: 0 0;
}";

            AssertLess(input, expected);
        }

        [Test]
        public void MixinsKeepImportantKeyword()
        {
            var input =
                @"
.important-mixin(@colour: #FFFFFF) {
  color: @colour !important;
}

important-rule {
  .important-mixin(#3f3f3f);
}
";

            var expected = @"
important-rule {
  color: #3f3f3f !important;
}
";

            AssertLess(input, expected);
        }

        [Test]
        public void ShortMixinDoesntMatchLongerSelectors()
        {
            var input =
            @"
#test {
  .mixin();
}

.mixin { color: red; }
.mixin:after, .dummy { color: green; }
.mixin .inner, .dummy { color: blue; }
";

            var expected =
                @"
#test {
  color: red;
}
.mixin {
  color: red;
}
.mixin:after, .dummy {
  color: green;
}
.mixin .inner, .dummy {
  color: blue;
}
";

            AssertLess(input, expected);
        }

        [Test]
        public void CanCallMixinFromWithinInnerRuleset()
        {
            var input =
            @"
#mybox {
  .box;
}
.box {
  .square();
}
.square() {
  width: 10px;
  height: 10px;
}
";

            var expected =
                @"
#mybox {
  width: 10px;
  height: 10px;
}
.box {
  width: 10px;
  height: 10px;
}
";

            AssertLess(input, expected);
        }

        [Test]
        public void CanResolveMixinsInSameScopeAsMixinDefinition()
        {
            var input =
            @"
#ns {
  .square() {
    width: 10px;
    height: 10px;
  }
  .box() {
    .square();
  }
}

#mybox {
  #ns > .box();
}
";

            var expected =
                @"
#mybox {
  width: 10px;
  height: 10px;
}
";

            AssertLess(input, expected);
        }

        [Test]
        public void CanResolveVariablesInSameScopeAsMixinDefinition()
        {
            var input =
            @"
#ns {
  @width: 10px;
  .box() {
    width: @width;
  }
}

#mybox {
  #ns > .box();
}
";

            var expected =
                @"
#mybox {
  width: 10px;
}
";

            AssertLess(input, expected);
        }

        [Test]
        public void IncludeAllMixinsInSameScope()
        {
            var input =
            @"
#ns {
  .mixin() { color: red; }
}
#ns {
  .mixin() { color: blue; }
  .box {
    #ns > .mixin();
  }
}
";

            var expected =
                @"
#ns .box {
  color: red;
  color: blue;
}
";

            AssertLess(input, expected);
        }
    }
}