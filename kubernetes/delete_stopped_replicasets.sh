#!/bin/bash

echo "Starting to delete all ReplicaSets with 0 desired, current, and ready replicas across all namespaces..."

kubectl get replicasets --all-namespaces | grep '0         0         0' | awk '{print $1 " " $2}' | while read -r namespace name; do
  echo "Deleting ReplicaSet $name in namespace $namespace..."
  kubectl delete replicasets "$name" --namespace="$namespace"
done

echo "Deletion process completed."
