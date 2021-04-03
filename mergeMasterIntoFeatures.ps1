$allBranches = "primeminister","financeandtrade"
foreach ($branch in $allBranches) {
	git checkout $branch
	git merge master
	git push
}
echo "Done"