const globalConst = "constant";
let globalLet = 42;
var globalVar = "var";
const regex = /[a-z]+/g;
const now = new Date();
const arr = [1, 3, 5];
const obj = {
	foo: "bar",
	baz: "beef"
};

for (let i = 0; i < 5; i++)
{
	const blockConst = 2;
	globalLet += i * blockConst;
}

// Test of column breakpoints
if (globalLet > 42) { globalLet = "hello"; globalLet += " world"; }

function outerFunction()
{
	const outerConst = 3;
	let outerLet = 4;
	var outerVar = 5;
	function innerFunction()
	{
		const innerConst = outerConst;
		let innerLet = outerLet;
		var innerVar = outerVar;

		if (outerVar > outerLet)
		{
			const innerBlockConst = 6;
			let innerBlockLet = 7;
			var innerBlockVar = 8;
		}
	}

	innerFunction();
}

outerFunction();