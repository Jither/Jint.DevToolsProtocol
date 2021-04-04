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

{
	let blockLet = 1;
	const blockConst = 2;
}

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

		{
			const innerBlockConst = 6;
			let innerBlockLet = 7;
			var innerBlockVar = 8;

			function innerMostFunction()
			{
				const innerMostConst = innerBlockConst;
				let innerMostLet = innerBlockLet;
				var innerMostVar = innerBlockVar;
			}
		}

		innerMostFunction();
	}

	innerFunction();
}

outerFunction();