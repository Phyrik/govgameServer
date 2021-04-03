$currentBranch = git rev-parse --abbrev-ref HEAD
$allBranches = "primeminister","financeandtrade"
foreach ($branch in $allBranches) {
	git checkout $branch
	git merge master
	git push
}
git checkout $currentBranch