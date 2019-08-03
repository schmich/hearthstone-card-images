update-cards:
	dotnet run --project update update-cards

download-images:
	dotnet run --project update download-images

copy-images:
	dotnet run --project update copy-images

check-images:
	dotnet run --project update check-images

create-manifests:
	dotnet run --project update create-manifests

.PHONY: update-cards download-images copy-images check-images create-manifests